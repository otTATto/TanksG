<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGiftLogsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('gift_logs', function (Blueprint $table) {
            $table->id(); // 自動インクリメントの主キー
            $table->unsignedBigInteger('user_id')->nullable(); // 配布対象ユーザーID
            $table->unsignedBigInteger('item_id'); // 配布アイテムID
            $table->integer('quantity'); // 配布アイテムの数量
            $table->boolean('is_global'); // 全体配布フラグ
            $table->timestamps(); // 作成日時と更新日時
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('gift_logs');
    }
}
