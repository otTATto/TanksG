<?php
namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Item extends Model
{
    use HasFactory;

    protected $fillable = ['name'];

    public function giftLogs()
    {
        return $this->hasMany(GiftLog::class);
    }
    public function users()
{
    return $this->hasMany(UserItem::class, 'item_id');
}

}
