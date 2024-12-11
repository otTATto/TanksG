<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class LoginBonusMasterSeeder extends Seeder
{
    public function run()
    {
        $bonuses = [
            ['day' => 1, 'item_id' => 1, 'quantity' => 10],
            ['day' => 2, 'item_id' => 2, 'quantity' => 5],
            ['day' => 3, 'item_id' => 3, 'quantity' => 3],
            ['day' => 4, 'item_id' => 4, 'quantity' => 2],
            ['day' => 5, 'item_id' => 5, 'quantity' => 20],
            ['day' => 6, 'item_id' => 1, 'quantity' => 15],
            ['day' => 7, 'item_id' => 2, 'quantity' => 10],
        ];

        foreach ($bonuses as $bonus) {
            DB::table('login_bonus_master')->insert($bonus);
        }
    }
}
