<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     *
     * @return void
     */
    public function run()
    {
        // シーディング：データベースに初期データを投入する

        // ItemsTableSeederを読み込むように指定
        $this->call(ItemsTableSeeder::class);
        
    }
}
