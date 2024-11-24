<?php
namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use App\Models\GiftLog;
use App\Models\Item; // アイテムモデル
use App\Models\GameUser; // ゲームユーザーモデル

class GiftLogController extends Controller
{
    public function index()
    {
        $giftLogs = GiftLog::with(['gameUser', 'item'])
            ->orderBy('created_at', 'desc')
            ->get();

        return view('admin.gift_logs.index', compact('giftLogs'));
    }

    // プレゼント配布フォームの表示
    public function create()
    {
        // アイテムのマスターデータを取得
        $items = Item::all();

        // ゲームユーザー（ユーザー）を取得
        $users = GameUser::all();

        return view('admin.gift_distribution', compact('items', 'users'));
    }

    // プレゼント配布の処理
    public function store(Request $request)
{
    // バリデーション
    try {
        $validated = $request->validate([
            'item_id' => 'required|exists:items,id',
            'quantity' => 'required|integer|min:1',
            'user_id' => 'nullable|exists:game_users,id',
        ]);
    } catch (\Illuminate\Validation\ValidationException $e) {
        \Log::error('バリデーションエラー:', $e->errors());
        return redirect()->back()->withErrors($e->errors());
    }

    // 配布処理のフラグ
    $isGlobal = $request->has('is_global') && $request->is_global;

    // ユーザーの取得
    $users = $isGlobal
        ? GameUser::all() // 全体配布の場合は全ユーザー
        : GameUser::where('id', $request->user_id)->get();

    if ($users->isEmpty()) {
        return redirect()->back()->withErrors(['error' => '配布対象のユーザーが存在しません。']);
    }

    // 配布処理
    try {
        // 全体配布の場合は1回だけ GiftLog を作成
        if ($isGlobal) {
            GiftLog::create([
                'user_id' => null,  // 全体配布は user_id を NULL に設定
                'item_id' => $request->item_id,
                'quantity' => $request->quantity,
                'is_global' => $isGlobal,
            ]);
        } else {
            // 特定ユーザー配布の場合は各ユーザーごとに GiftLog を作成
            foreach ($users as $user) {
                GiftLog::create([
                    'user_id' => $user->id,
                    'item_id' => $request->item_id,
                    'quantity' => $request->quantity,
                    'is_global' => $isGlobal,
                ]);
            }
        }

        \Log::info('プレゼント配布が完了しました。');
        return redirect()->route('admin.gift_distribution.create')->with('success', 'プレゼントの配布が完了しました');
    } catch (\Exception $e) {
        \Log::error('プレゼント配布処理中にエラーが発生しました: ' . $e->getMessage());
        return redirect()->back()->withErrors(['error' => '配布処理中にエラーが発生しました。']);
    }
}

}
