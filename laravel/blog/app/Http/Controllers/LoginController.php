<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Carbon;
use App\Models\GameUser;

class LoginController extends Controller
{
    /**
     * ユーザーのログイン処理
     *
     * @param Request $request
     * @param int $id ユーザーID
     * @return \Illuminate\Http\JsonResponse
     */
    public function login(Request $request, $id)
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

        // 最終ログイン日が今日でなければ更新
        if (empty($user->last_login_date) || $user->last_login_date->toDateString() != $today) {
            $user->last_login_date = $now;
            $user->save();
        }

        // 今日のボーナス受取状況確認
        $alreadyReceived = $user->last_bonus_date && $user->last_bonus_date->toDateString() == $today;
        $canReceiveBonus = !$alreadyReceived;

        return response()->json([
            'can_receive_bonus' => $canReceiveBonus,
            'day' => $user->bonus_day_count
        ]);
    }
}
