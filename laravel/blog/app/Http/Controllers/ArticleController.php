<?php

namespace App\Http\Controllers;

use App\Models\Article;
use Illuminate\Http\Request;

class ArticleController extends Controller
{
    /**
     * Display a listing of the resource.
     *
     * @return \Illuminate\Http\Response
     */
    public function index()
    {
        //一覧を表示する
        $articles=Article::all();
        $data=['articles'=>$articles];
        return view('articles.index',$data);
    }

    /**
     * Show the form for creating a new resource.
     *
     * @return \Illuminate\Http\Response
     */
    public function create()
    {
        $article=new Article();
        $data=['article'=> $article];
        return view('articles.create',$data);
        //
    }

    /**
     * Store a newly created resource in storage.
     *
     * @param  \Illuminate\Http\Request  $request
     * @return \Illuminate\Http\Response
     */
    public function store(Request $request)
    {
        //
        $this->validate($request,[
            'title'=>'required|max:255',
            'body'=>'required'
        ]);
        $article=new Article();
        $article->user_id = \Auth::id(); // 一行追加
        $article->title=$request->title;
        $article->body=$request->body;
        $article->save();
        return redirect(route('articles.index'));
    }

    /**
     * Display the specified resource.
     *
     * @param  \App\Models\Article  $article
     * @return \Illuminate\Http\Response
     */
    public function show(Article $article)
    {
        $data=['article'=>$article];//
        return view('articles.show',$data);
    }

    /**
     * Show the form for editing the specified resource.
     *
     * @param  \App\Models\Article  $article
     * @return \Illuminate\Http\Response
     */
    public function edit(Article $article)
    {
        $data=['article'=>$article];//
        return view('articles.edit',$data);
    }

    /**
     * Update the specified resource in storage.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \App\Models\Article  $article
     * @return \Illuminate\Http\Response
     */
    public function update(Request $request, Article $article)
    {
        $this->validate($request, [
            'title' => 'required|max:255',
            'body' => 'required'
        ]);
        $article->title = $request->title;
        $article->body = $request->body;
        $article->save();
        return redirect(route('articles.show', $article));
        //
    }

    /**
     * Remove the specified resource from storage.
     *
     * @param  \App\Models\Article  $article
     * @return \Illuminate\Http\Response
     */
    public function destroy(Article $article)
    {
        $article->delete();//
        return redirect(route('articles.index'));
    }
}
