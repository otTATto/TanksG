<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;

class ItemController extends Controller
{
    public function index()
    {
        // ToDo: ユーザーごとのアイテム一覧を取得 -> view() の compact() に追加

        // ToDo: ユーザー検索機能を実装

        // HTML を返す
        return view('admin.items.index');
    }
}
