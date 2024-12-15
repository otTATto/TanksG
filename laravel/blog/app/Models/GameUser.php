<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameUser extends Model
{
    use HasFactory;
    
    protected $table = 'game_users';
    
    // fillableは必要なカラムを指定（nameやis_suspendedなど）
    protected $fillable = [
        'uuid',
        'name',
        'is_suspended',
        'last_login_date', 
        'last_bonus_date', 
        'bonus_day_count'
    ];
    
    protected $casts = [
        'is_suspended' => 'boolean'
    ];

    // 日付型として扱いたいカラムを指定
    protected $dates = [
        'last_login_date',
        'last_bonus_date'
    ];

    public function stats()
    {
        return $this->hasOne(PlayerStat::class, 'game_user_id');
    }

    public function items()
    {
        return $this->hasMany(UserItem::class, 'user_id');
    }

    public function addItem($itemId, $quantity)
    {
        $userItem = $this->items()->where('item_id', $itemId)->first();
        if (!$userItem) {
            $userItem = new UserItem();
            $userItem->user_id = $this->id;
            $userItem->item_id = $itemId;
            $userItem->quantity = 0;
        }

        $userItem->quantity += $quantity;
        $userItem->save();
    }
}
