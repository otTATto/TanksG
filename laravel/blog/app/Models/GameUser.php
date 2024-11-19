<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameUser extends Model
{
    use HasFactory;
    protected $table = 'game_users'; // テーブル名の指定
    protected $fillable = ['name', 'is_suspended']; // 許可するカラム
}
