<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AddLoginBonusColumnsToGameUsersTable extends Migration
{
    public function up()
    {
        Schema::table('game_users', function (Blueprint $table) {
            $table->date('last_login_date')->nullable()->after('is_suspended'); 
            $table->date('last_bonus_date')->nullable()->after('last_login_date');
            $table->integer('bonus_day_count')->default(1)->after('last_bonus_date');
        });
    }

    public function down()
    {
        Schema::table('game_users', function (Blueprint $table) {
            $table->dropColumn(['last_login_date', 'last_bonus_date', 'bonus_day_count']);
        });
    }
}
