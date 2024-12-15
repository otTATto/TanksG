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

        // Note: Users（管理者）および GameUsers（ゲームユーザー）のマスターデータを真っ先に登録する必要あり

        // UsersTableSeeder を読み込むように指定
        $this->call(UsersTableSeeder::class);
        // GameUsersSeeder を読み込むように指定
        $this->call(GameUsersTableSeeder::class);

        // ArticlesTableSeeder を読み込むように指定
        $this->call(ArticlesTableSeeder::class);
        // ItemsTableSeeder を読み込むように指定
        $this->call(ItemsTableSeeder::class);
        // UserItemsTableSeeder を読み込むように指定
        $this->call(UserItemsTableSeeder::class);
        $this->call(LoginBonusMasterSeeder::class);
        
    }
}
