<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
use App\Models\UserItem;
use Illuminate\Http\Request;

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
            ->select('items.name', 'user_items.quantity')
            ->get();

        return response()->json($items);
    }
}
