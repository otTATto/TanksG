<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;

use App\Models\Article;

class MyDataController extends Controller
{
    public function getInfo()
    {
        $users =Article::all();
        return response()->json($users);
    }
}
