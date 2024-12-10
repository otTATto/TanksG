<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Carbon;
use App\Models\GameUser;
use App\Models\LoginBonusMaster;

class LoginBonusController extends Controller
{
    /**
     * ログインボーナスの受取処理
     *
     * @param Request $request
     * @param int $id ユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function receive(Request $request, $id)
    {
        // ユーザーIDが有効か確認
        if (!is_numeric($id) || $id <= 0) {
            return response()->json(['message' => 'Invalid user ID'], 400);
        }

        // 指定されたユーザーIDでユーザーを取得
        $user = GameUser::find($id);

        if (!$user) {
            return response()->json(['message' => 'User not found'], 404);
        }

        $now = Carbon::now('Asia/Tokyo');
        $today = $now->toDateString();

        // 今日既に受け取っていたらエラー
        if ($user->last_bonus_date && $user->last_bonus_date->toDateString() == $today) {
            return response()->json(['message' => 'Already received today'], 400);
        }

        $day = $user->bonus_day_count;
        $bonus = LoginBonusMaster::where('day', $day)->first();

        if (!$bonus) {
            return response()->json(['message' => 'No bonus data found.'], 400);
        }

        // アイテム付与
        $user->addItem($bonus->item_id, $bonus->quantity);

        // ボーナス情報更新
        $user->last_bonus_date = $today;
        $nextDay = $day + 1;
        if ($nextDay > 7) {
            $nextDay = 1;
        }
        $user->bonus_day_count = $nextDay;
        $user->save();

        return response()->json([
            'message' => 'Bonus received successfully.',
            'item_id' => $bonus->item_id,
            'quantity' => $bonus->quantity,
            'next_day' => $nextDay
        ]);
    }
}
