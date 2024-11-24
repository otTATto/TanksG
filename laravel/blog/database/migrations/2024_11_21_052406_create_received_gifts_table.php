<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateReceivedGiftsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('received_gifts', function (Blueprint $table) {
            $table->id();
            $table->unsignedBigInteger('user_id'); // ユーザーID
            $table->unsignedBigInteger('gift_log_id'); // ギフトログID
            $table->timestamp('received_at'); // 受け取った日時
    
            // 外部キー制約
            $table->foreign('user_id')->references('id')->on('game_users')->onDelete('cascade');
            $table->foreign('gift_log_id')->references('id')->on('gift_logs')->onDelete('cascade');
        });
    }
    
    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('received_gifts');
    }
}
