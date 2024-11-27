<?php
namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use App\Models\GameUser;
use Illuminate\Http\Request;

class GameUserController extends Controller
{
    // ゲームユーザー一覧を表示
    public function index()
    {
        $gameUsers = GameUser::all(); // すべてのゲームユーザーを取得
        return view('admin.game_users.index', compact('gameUsers'));
    }

    // ゲームユーザーのアカウント停止/復活を切り替え
    public function toggleSuspend($id)
    {
        // ゲームユーザーをIDで検索し、存在しなければ404エラー
        $gameUser = GameUser::findOrFail($id);

        // アカウント状態を反転
        $gameUser->is_suspended = !$gameUser->is_suspended;

        // データを保存
        $gameUser->save();

        // 処理後、ゲームユーザー一覧ページにリダイレクト
        return redirect()->route('admin.game_users.index')->with('status', 'Account status updated successfully!');
    }
}
