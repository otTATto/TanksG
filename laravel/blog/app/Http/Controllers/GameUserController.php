<?php

namespace App\Http\Controllers;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
use Illuminate\Http\Request;
use Illuminate\Support\Str;

class GameUserController extends Controller
{
    /**
     * 新規ゲームユーザーを作成
     * 
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function store(Request $request)
    {
        try {
            
            $gameUser = GameUser::create([
                'uuid' => Str::uuid(),
                'name' => $request->input('name', 'NONAME'),
                'is_suspended' => false
            ]);
            
            return response()->json([
                'success' => true,
                'id' => $gameUser->id,
                'uuid' => $gameUser->uuid
            ]);
            
        } catch (\Exception $e) {
            // エラー情報を詳細にログに記録
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

    /**
     * ゲームユーザーのアカウント停止状態を確認
     * 
     * @param  string  $uuid  ゲームユーザーUUID
     * @return \Illuminate\Http\JsonResponse
     */
    public function checkSuspended($uuid)
    {
        $gameUser = GameUser::where('uuid', $uuid)->first();

        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        return response()->json(['is_suspended' => $gameUser->is_suspended]);
    }

    /**
     * UUIDからプレイヤー情報を取得
     * 
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function getPlayerInformation(Request $request)
    {
        $uuid = $request->input('uuid');
        
        $gameUser = GameUser::where('uuid', $uuid)->first();

        if (!$gameUser) {
            return response()->json(['error' => 'User not found'], 404);
        }

        return response()->json([
            'id' => $gameUser->id,
            'playername' => $gameUser->name
        ]);
    }
}