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

        return view('admin.items.index', compact('userItems', 'gameUsers', 'items'));
    }

    // アイテムの追加
    public function store(Request $request)
    {
        // リクエストからアイテム名を取得
        $itemName = $request->input('item_name');

        // アイテム名が空の場合はリダイレクト
        if (empty($itemName)) {
            return redirect()->route('admin.items.index');
        }

        // アイテムを追加
        Item::create(['name' => $itemName]);

        // アイテム追加後にリダイレクト
        return redirect()->route('admin.items.index');
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