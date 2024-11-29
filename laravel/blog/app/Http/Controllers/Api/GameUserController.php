<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
use App\Models\PlayerStat;
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
            \Log::info('Update ranking request:', $request->all());
            
            $id = $request->input('id');
            $isPlayerWin = $request->input('ifplayerwin');
            
            // GameUserの存在確認
            $gameUser = GameUser::find($id);
            if (!$gameUser) {
                return response()->json([
                    'success' => false,
                    'error' => 'User not found'
                ], 404);
            }

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
                                 $stat->ranking = $rank++;
                                 $stat->save();
                             }
                         });
                
                \DB::commit();
                
                // 上位10名を取得
                $players = GameUser::join('player_stats', 'game_users.id', '=', 'player_stats.game_user_id')
                                  ->orderBy('player_stats.ranking', 'asc')
                                  ->take(10)
                                  ->get([
                                      'game_users.id',
                                      'game_users.name as playername',
                                      'player_stats.ranking',
                                      'player_stats.winrate',
                                      'player_stats.wincount',
                                      'player_stats.losecount'
                                  ])
                                  ->toArray();
                
                return response()->json([
                    'success' => true,
                    'playerDatas' => $players
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
}
