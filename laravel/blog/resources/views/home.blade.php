@extends('layouts.app')
@section('content')
<h1 class="page-heading">マイページ</h1>
<p>ようこそ、{{ Auth::user()->name }}さん｜<a href="{{ route('articles.create') }}">お知らせを書く</a></p>
@include('articles.articles')
@endsection()