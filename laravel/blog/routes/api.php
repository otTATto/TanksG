<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\MyDataController;
use App\Http\Controllers\Api\ContactController;
use App\Http\Controllers\Api\GameUserController;
use App\Http\Controllers\Api\GiftController;
use App\Http\Controllers\LoginController;
use App\Http\Controllers\LoginBonusController;



/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
    return $request->user();
});

Route::get('/blog', [MyDataController::class, 'getInfo']);
Route::apiResource('contact', ContactController::class);

// 返信を追加するエンドポイント
Route::post('contact/{contact_id}/replies', [ContactController::class, 'addReply']);

// ゲームユーザーに関するAPI

// アカウント停止状態を確認するエンドポイント
Route::get('game-users/{id}/check-suspended', [GameUserController::class, 'checkSuspended']);

// スタミナを取得するエンドポイント
Route::get('game-users/{id}/stamina/get', [GameUserController::class, 'getStamina']);
// スタミナを回復するエンドポイント
Route::post('game-users/{id}/stamina/recover', [GameUserController::class, 'recoverStamina']);
// スタミナを消費するエンドポイント
Route::post('game-users/{id}/stamina/consume', [GameUserController::class, 'consumeStamina']);

// 所有アイテム一覧を取得するエンドポイント
Route::get('game-users/{id}/items', [GameUserController::class, 'getUserItems']);   
// アイテムを使用するエンドポイント
Route::delete('game-users/{userId}/items/{itemId}/use', [GameUserController::class, 'useItem']);  

// プレゼントを送るエンドポイント          
Route::post('/grant-item', [GiftController::class, 'grantItem']);
// プレゼント一覧を取得するエンドポイント
Route::get('/presents/{user_id}', [GiftController::class, 'getUserPresents']); 
// プレゼントを受け取るエンドポイント
Route::post('/presents/received/{user_id}/{present_id}', [GiftController::class, 'receivePresentById']);
Route::post('/gameusers', [GameUserController::class, 'store']); // 新規ユーザー登録

// プレイヤー情報取得のルートを追加
Route::post('/getplayerinformation', [GameUserController::class, 'getPlayerInformation']);

// 名前変更のルートを修正
Route::post('gameusers/update-name', [GameUserController::class, 'updateName']);

// ランキング更新のルートを追加
Route::post('/updateranking', [GameUserController::class, 'updateRanking']);

// ユーザーのログインAPI
Route::get('/login/{id}', [LoginController::class, 'login']);

// ログインボーナス受取API
Route::post('/bonus/receive/{id}', [LoginBonusController::class, 'receive']);
Route::get('/login-bonuses', [LoginBonusController::class, 'getAllBonuses']);
