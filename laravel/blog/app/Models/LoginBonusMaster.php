<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class LoginBonusMaster extends Model
{
    protected $table = 'login_bonus_master';

    protected $fillable = ['day', 'item_id', 'quantity'];

    public $timestamps = true;

    public function item()
    {
        return $this->belongsTo(Item::class, 'item_id');
    }
}
