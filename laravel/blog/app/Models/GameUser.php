<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameUser extends Model
{
    use HasFactory;
    protected $table = 'game_users'; // テーブル名の指定
    protected $fillable = [
        'uuid',
        'name',
        'is_suspended',
        'ranking',
        'winrate',
        'wincount',
        'losecount'
    ];  // 許可するカラム
    protected $casts = [
        'is_suspended' => 'boolean',
        'winrate' => 'integer',
        'wincount' => 'integer',
        'losecount' => 'integer'
    ];
    public function items()
{
    return $this->hasMany(UserItem::class, 'user_id');
}

}
