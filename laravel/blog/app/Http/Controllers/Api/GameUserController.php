<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
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
            
            $gameUser = GameUser::create([
                'uuid' => Str::uuid(),
                'name' => $request->input('name', 'NONAME'),
                'is_suspended' => false,
                'ranking' => 0,
                'winrate' => 0,
                'wincount' => 0,
                'losecount' => 0
            ]);
            
            \Log::info('Created user:', $gameUser->toArray());
            
            return response()->json([
                'success' => true,
                'id' => $gameUser->id,
                'uuid' => $gameUser->uuid
            ]);
            
        } catch (\Exception $e) {
            \Log::error('Error creating user:', [
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
}
