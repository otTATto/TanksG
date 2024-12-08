<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class PlayerStat extends Model
{
    use HasFactory;

    protected $fillable = [
        'game_user_id',
        'ranking',
        'winrate',
        'wincount',
        'losecount'
    ];

    protected $casts = [
        'winrate' => 'float',
    ];

    public function gameUser()
    {
        return $this->belongsTo(GameUser::class, 'game_user_id');
    }
} 