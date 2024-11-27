<?php
namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GiftLog extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'item_id', 'quantity', 'is_global'];

    // GameUserモデルとの関連付け
    public function gameUser()
    {
        return $this->belongsTo(GameUser::class, 'user_id');
    }

    // Itemモデルとの関連付け
    public function item()
    {
        return $this->belongsTo(Item::class);
    }
}
