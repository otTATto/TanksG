<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
use App\Models\PlayerStat;
use App\Models\UserItem;
use Illuminate\Http\Request;
use Illuminate\Support\Str;

class GameUserController extends Controller
{
    /**
     * ゲームユーザーのアカウント停止状態を確認
     * 
     * @param  int  $id  ゲームユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function checkSuspended($id)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($id);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // アカウント停止状態を返す
        return response()->json(['is_suspended' => $gameUser->is_suspended]);
    }

    public function store(Request $request)
    {
        try {
            \Log::info('Request data:', $request->all());
            
            \DB::beginTransaction();
            
            $gameUser = GameUser::create([
                'uuid' => Str::uuid(),
                'name' => $request->input('name', 'NONAME'),
                'is_suspended' => false
            ]);
            
            // PlayerStatを同時に作成
            $gameUser->stats()->create([
                'ranking' => 0,
                'winrate' => 0,
                'wincount' => 0,
                'losecount' => 0
            ]);
            
            \DB::commit();
            
            return response()->json([
                'success' => true,
                'id' => $gameUser->id,
                'uuid' => $gameUser->uuid
            ]);
            
        } catch (\Exception $e) {
            \DB::rollBack();
            \Log::error('User creation failed:', [
                'message' => $e->getMessage(),
                'trace' => $e->getTraceAsString()
            ]);
            
            return response()->json([
                'success' => false,
                'error' => $e->getMessage()
            ], 500);
        }
    }

    public function getPlayerInformation(Request $request)
    {
        try {
            // リクエストの内容をログに出力
            \Log::info('GetPlayerInformation request:', $request->all());
            
            $uuid = $request->input('uuid');
            
            if (!$uuid) {
                \Log::error('UUID is missing');
                return response()->json([
                    'error' => 'UUID is required'
                ], 400);
            }

            $gameUser = GameUser::where('uuid', $uuid)->first();

            if (!$gameUser) {
                \Log::error('User not found for UUID: ' . $uuid);
                return response()->json([
                    'error' => 'User not found'
                ], 404);
            }

            \Log::info('User found:', $gameUser->toArray());

            return response()->json([
                'id' => $gameUser->id,
                'playername' => $gameUser->name
            ]);
            
        } catch (\Exception $e) {
            \Log::error('Error in getPlayerInformation:', [
                'message' => $e->getMessage(),
                'trace' => $e->getTraceAsString()
            ]);
            
            return response()->json([
                'error' => 'Internal server error'
            ], 500);
        }
    }

    public function updateName(Request $request)
    {
        try {
            \Log::info('Update name request:', $request->all());
            
            $id = $request->input('id');
            $newName = $request->input('playername');
            
            $gameUser = GameUser::find($id);
            
            if (!$gameUser) {
                return response()->json([
                    'success' => false,
                    'error' => 'User not found'
                ], 404);
            }
            
            $gameUser->name = $newName;
            $gameUser->save();
            
            return response()->json([
                'success' => true
            ]);
            
        } catch (\Exception $e) {
            \Log::error('Error updating name:', [
                'message' => $e->getMessage(),
                'trace' => $e->getTraceAsString()
            ]);
            
            return response()->json([
                'success' => false,
                'error' => $e->getMessage()
            ], 500);
        }
    }

    public function updateRanking(Request $request)
    {
        try {
            $id = $request->input('id');
            $isPlayerWin = $request->input('ifplayerwin');
            
            // 更新前のランクを取得
            $previousRank = PlayerStat::where('game_user_id', $id)
                ->value('ranking');

            \DB::beginTransaction();
            try {
                // PlayerStatの取得または作成
                $playerStat = PlayerStat::firstOrCreate(
                    ['game_user_id' => $id],  // game_user_idで検索
                    [
                        'ranking' => 0,
                        'winrate' => 0,
                        'wincount' => 0,
                        'losecount' => 0
                    ]
                );
                
                // 勝敗を更新
                if ($isPlayerWin == 1) {
                    $playerStat->wincount++;
                } else {
                    $playerStat->losecount++;
                }
                
                // 勝率を計算
                $totalGames = $playerStat->wincount + $playerStat->losecount;
                if ($totalGames > 0) {
                    $playerStat->winrate = round(($playerStat->wincount / $totalGames) * 100, 2);
                }
                
                $playerStat->save();
                
                // 全プレイヤーのランキングを更新
                $rank = 1;
                PlayerStat::orderBy('winrate', 'desc')
                         ->orderBy('wincount', 'desc')
                         ->chunk(100, function($stats) use (&$rank) {
                             foreach ($stats as $stat) {
                                 $total = $stat->wincount + $stat->losecount;
                                 if($total > 9){
                                 $stat->ranking = $rank++;
                                 $stat->save();
                                }
                             }
                         });
                
                // 更新後のプレイヤー情報を取得
                $currentPlayer = GameUser::join('player_stats', 'game_users.id', '=', 'player_stats.game_user_id')
                                          ->where('game_users.id', $id)
                                          ->select([
                                              'game_users.id',
                                              'game_users.name as playername',
                                              'player_stats.ranking',
                                              \DB::raw('CAST(player_stats.winrate AS DECIMAL(5,2)) as winrate'),
                                              'player_stats.wincount',
                                              'player_stats.losecount'
                                          ])
                                          ->first();
                
                if (!$currentPlayer) {
                    throw new \Exception('Current player not found');
                }
                
                // ランクアップしたかどうかのみ確認
                $isRankUp = $previousRank > $currentPlayer->ranking;
                
                // 上位10名を取得
                $players = GameUser::join('player_stats', 'game_users.id', '=', 'player_stats.game_user_id')
                   ->where('player_stats.ranking', '>', 0) // ranking が 0 より大きいものを取得
                   ->orderBy('player_stats.ranking', 'asc')
                   ->take(10)
                   ->get([
                       'game_users.id',
                       'game_users.name as playername',
                       'player_stats.ranking',
                       \DB::raw('CAST(player_stats.winrate AS DECIMAL(5,2)) as winrate'),
                       'player_stats.wincount',
                       'player_stats.losecount'
                   ])
                   ->toArray();
                
                \DB::commit();
                
                return response()->json([
                    'success' => true,
                    'playerDatas' => $players,
                    'currentPlayer' => $currentPlayer,
                    'isRankUp' => $isRankUp ?? false
                ]);
                
            } catch (\Exception $e) {
                \DB::rollBack();
                throw $e;
            }
            
        } catch (\Exception $e) {
            \Log::error('Error in updateRanking:', [
                'message' => $e->getMessage(),
                'trace' => $e->getTraceAsString()
            ]);
            
            return response()->json([
                'success' => false,
                'error' => $e->getMessage()
            ], 500);
        }
    }
    /**
     * ゲームユーザーのスタミナを取得
     * 
     * @param  int  $id  ゲームユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function getStamina($id)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($id);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // 現在のスタミナを返す
        return response()->json($gameUser->stamina);
    }

    /**
     * ゲームユーザーのスタミナを回復
     * 
     * @param  int  $id  ゲームユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function recoverStamina($id)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($id);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // スタミナが最大値の場合は回復しない
        if ($gameUser->stamina === 5) {
            // 現在のスタミナを返す
            return response()->json($gameUser->stamina);
        }

        // スタミナをインクリメント
        $gameUser->stamina += 1;

        // 変更をデータベースに保存
        $gameUser->save();

        // 現在のスタミナを返す
        return response()->json($gameUser->stamina);
    }

    /**
     * ゲームユーザーのスタミナを消費
     * 
     * @param  int  $id  ゲームユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function consumeStamina($id)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($id);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // スタミナが0の場合はデクリメントしない
        if ($gameUser->stamina === 0) {
            // 現在のスタミナを返す
            return response()->json($gameUser->stamina);
        }

        // スタミナをデクリメント
        $gameUser->stamina -= 1;

        // 変更をデータベースに保存
        $gameUser->save();

        // 現在のスタミナを返す
        return response()->json($gameUser->stamina);
    }

    /**
     * ゲームユーザーの所有アイテム一覧（アイテム名・量）を取得
     */
    public function getUserItems($id)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($id);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // ゲームユーザーの所有アイテム一覧を取得
        $items = UserItem::where('user_id', $id)
            ->join('items', 'user_items.item_id', '=', 'items.id')
            ->select('items.id', 'items.name', 'items.description', 'items.type', 'user_items.quantity')
            ->get();

        return response()->json($items);
    }

    /** 
     * ゲームユーザーの所有アイテムを1つ減らす
     * 
     *  @param  int  $userId  ゲームユーザーID
     *  @param  int  $itemId  アイテムID
     *  @return \Illuminate\Http\JsonResponse
     */
    public function useItem($userId, $itemId)
    {
        // ゲームユーザーをIDで検索
        $gameUser = GameUser::find($userId);

        // ユーザーが見つからない場合は404エラー
        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        // アイテムをIDで検索
        $item = UserItem::where('user_id', $userId)
            ->where('item_id', $itemId)
            ->first();

        // アイテムが見つからない場合は404エラー
        if (!$item) {
            return response()->json(['error' => 'Item not found'], 404);
        }

        // アイテムの量を1つ減らす
        $item->quantity -= 1;
        $item->save();

        return response()->json(['message' => 'Item used'], 204);
    }

}
