<?php
// app/Http/Middleware/AdminMiddleware.php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;

class AdminMiddleware
{
    public function handle(Request $request, Closure $next)
    {
        // ここで管理者かどうかを確認します
        if (Auth::check() && Auth::user()->is_admin) {
            return $next($request);
        }

        // 管理者でなければ、403エラーやリダイレクトなどを行うことができます
        return redirect()->route('home'); // 例えばホームページにリダイレクト
    }
}
