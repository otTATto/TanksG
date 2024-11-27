<?php
namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use App\Models\UserItem;
use Illuminate\Support\Facades\DB;

class GiftController extends Controller
{
    // アイテムをユーザーに直接付与する
    public function grantItem(Request $request)
    {
        $validated = $request->validate([
            'user_id' => 'required|exists:game_users,id',
            'item_id' => 'required|exists:items,id',
            'quantity' => 'required|integer|min:1',
        ]);

        $userItem = UserItem::firstOrCreate(
            ['user_id' => $request->user_id, 'item_id' => $request->item_id],
            ['quantity' => 0]
        );

        $userItem->quantity += $request->quantity;
        $userItem->save();

        return response()->json(['message' => 'アイテムが正常に付与されました']);
    }

    // プレゼントBOXのリストを取得
    public function getUserPresents($user_id)
    {
        $presents = DB::table('gift_logs')
            ->join('items', 'gift_logs.item_id', '=', 'items.id')
            ->leftJoin('received_gifts', function ($join) use ($user_id) {
                $join->on('gift_logs.id', '=', 'received_gifts.gift_log_id')
                     ->where('received_gifts.user_id', '=', $user_id);
            })
            ->whereNull('received_gifts.id') // 未受け取りのプレゼント
            ->where(function ($query) use ($user_id) {
                $query->where('gift_logs.user_id', '=', $user_id)
                      ->orWhere('gift_logs.is_global', '=', 1);
            })
            ->select(
                'gift_logs.id as gift_id',
                'items.name as item_name',
                'gift_logs.quantity',
                'gift_logs.created_at as distributed_at'
            )
            ->orderBy('gift_logs.created_at', 'desc')
            ->get();

        if ($presents->isEmpty()) {
            return response()->json(['message' => 'プレゼントはありません'], 200);
        }

        return response()->json($presents, 200);
    }
    public function receivePresentById($user_id, $present_id)
    {
        // Check if the gift exists
        $gift = DB::table('gift_logs')
            ->where('id', $present_id)
            ->where(function ($query) use ($user_id) {
                $query->where('user_id', $user_id)
                      ->orWhere('is_global', 1); // Include globally distributed gifts
            })
            ->first();
    
        if (!$gift) {
            return response()->json(['message' => 'This gift is not available for you to receive'], 404);
        }
    
        // Check if the gift has already been received
        $received = DB::table('received_gifts')
            ->where('user_id', $user_id)
            ->where('gift_log_id', $present_id)
            ->exists();
    
        if ($received) {
            return response()->json(['message' => 'You have already received this gift'], 400);
        }
    
        // Save the receipt record
        DB::table('received_gifts')->insert([
            'user_id' => $user_id,
            'gift_log_id' => $present_id,
            'received_at' => now(),
        ]);
    
        // Grant the item to the user
        $userItem = UserItem::firstOrCreate(
            ['user_id' => $user_id, 'item_id' => $gift->item_id],
            ['quantity' => 0]
        );
        $userItem->quantity += $gift->quantity;
        $userItem->save();
    
        return response()->json(['message' => 'The gift has been successfully received'], 200);
    }
    
}
