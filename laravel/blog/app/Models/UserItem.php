<?php
namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class UserItem extends Model
{
    use HasFactory;
    protected $table = 'user_items'; // テーブル名の指定

    protected $fillable = ['user_id', 'item_id', 'quantity'];

    public function user()
    {
        return $this->belongsTo(GameUser::class);
    }

    public function item()
    {
        return $this->belongsTo(Item::class);
    }
}
