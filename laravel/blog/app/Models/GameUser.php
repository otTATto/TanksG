<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameUser extends Model
{
    use HasFactory;
    
    protected $table = 'game_users';
    
    protected $fillable = [
        'uuid',
        'name',
        'is_suspended'
    ];
    
    protected $casts = [
        'is_suspended' => 'boolean'
    ];

    public function stats()
    {
        return $this->hasOne(PlayerStat::class, 'game_user_id');
    }

    public function items()
    {
        return $this->hasMany(UserItem::class, 'user_id');
    }
}
