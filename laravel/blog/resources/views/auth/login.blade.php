
@extends('layouts.app')
@section('content')
<h1>ログイン</h1>
@include('commons.errors')
<form action="{{ route('login') }}" method="post">
    @csrf 
    <dl class="form-list">
        <dt>メールアドレス</dt>
        <dd><input type="email" name="email" value="{{ old('email') }}"></dd>
        <dt>パスワード</dt>
        <dd><input type="password" name="password"></dd>
    </dl>
    <button type="submit">ログイン</button>
    <a href="/">キャンセル</a>
</form>
@endsection()