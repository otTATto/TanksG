<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use App\Models\UserItem;
use App\Models\GameUser;
use App\Models\Item;
use Illuminate\Http\Request;

class ItemController extends Controller
{
    public function index()
    {
        // ユーザーごとのアイテム一覧を取得
        $userItems = UserItem::all();
        // すべてのゲームユーザーを取得
        $gameUsers = GameUser::all();
        // すべてのアイテム一覧を取得
        $items = Item::all();

        // ToDo: ユーザー検索機能を実装

        // HTML を返す（取得した userItems, gameUsers, items を渡す）
        return view('admin.items.index', compact('userItems', 'gameUsers', 'items'));
    }

    // ユーザー検索
    public function search(Request $request)
    {
        $userId = $request->input('user_id');

        // user_id が空の場合は全ての UserItem を取得
        if (empty($userId)) {
            $userItems = UserItem::all();
        } else {
            // 指定されたユーザーIDのアイテム一覧を取得
            $userItems = UserItem::where('user_id', $userId)->get();
        }
        
        // すべてのゲームユーザーを取得
        $gameUsers = GameUser::all();
        // すべてのアイテム一覧を取得
        $items = Item::all();

        return view('admin.items.index', compact('userItems', 'gameUsers', 'items'));
    }
}
