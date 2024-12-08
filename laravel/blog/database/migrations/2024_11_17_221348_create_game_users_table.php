<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGameUsersTable extends Migration // クラス名も変更
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('game_users', function (Blueprint $table) { // テーブル名を複数形に変更
            $table->id(); // ユーザーID
            $table->uuid('uuid')->unique();
            $table->string('name')->default('NoName');
            $table->boolean('is_suspended')->default(false); // アカウント停止情報
            $table->timestamps(); // 作成日時・更新日時
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('game_users'); // 同じく複数形に変更
    }
}
