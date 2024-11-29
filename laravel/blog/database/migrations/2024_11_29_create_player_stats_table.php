<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up()
    {
        Schema::create('player_stats', function (Blueprint $table) {
            $table->id();
            $table->foreignId('game_user_id')
                  ->constrained('game_users')
                  ->onDelete('cascade');
            $table->integer('ranking')->default(0);
            $table->decimal('winrate', 5, 2)->default(0);
            $table->integer('wincount')->default(0);
            $table->integer('losecount')->default(0);
            $table->timestamps();

            $table->unique('game_user_id');
        });
    }

    public function down()
    {
        Schema::dropIfExists('player_stats');
    }
};