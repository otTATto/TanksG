<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>管理画面 - TanksG </title>
    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
    <!-- Google Fonts -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Zen+Kaku+Gothic+New:wght@300;400;500;700;900&family=Zen+Maru+Gothic:wght@300;400;500;700;900&display=swap" rel="stylesheet">
    <!-- オリジナルCSS -->
    <link rel="stylesheet" href="/main.css">
</head>
<body>
    <!-- Bootstrap -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js" integrity="sha384-geWF76RCwLtnZ8qwWowPQNguL3RmwHVBC9FhGdlKrxdiJJigb/j/68SIy3Te4Bkz" crossorigin="anonymous"></script>
    <header>
        <a href="/" class="site-title" style="color: white; text-decoration: none;" >Laravel</a>
        <nav class="tab">
            <ul>
                @if (Auth::check())
                <li><a class="tab-item {{ Request::is('home') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('home') }}">マイページ</a></li>
                <li><a class="tab-item {{ Request::is('articles') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('articles.index') }}">お知らせ</a></li>
                <li><a class="tab-item {{ Request::is('contact') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('contact.index') }}">お問い合わせ</a></li>
                
                @if (Auth::user()->is_admin) <!-- 管理者のみ表示 -->
                <li><a class="tab-item {{ Request::is('admin/game_users') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('admin.game_users.index') }}">ゲームユーザー管理</a></li>
                <li><a class="tab-item {{ Request::is('admin/items') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('admin.items.index') }}">アイテム管理</a></li>
                <li><a class="tab-item {{ Request::is('admin/gift-logs') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('admin.gift_logs.index') }}">プレゼント配布履歴</a></li>
                <li><a class="tab-item {{ Request::is('admin/gift-distribution') ? ' here-text' : 'not-here-text'}}" style="text-decoration: none;" href="{{ route('admin.gift_distribution.create') }}">プレゼント配布</a></li>
                @endif
                
                <li>
                    <form onsubmit="return confirm('ログアウトしますか？')" action="{{ route('logout') }}" method="post">
                        @csrf
                        <button type="submit" class="btn btn-outline-danger">ログアウト</button>
                    </form>
                </li>
                @else 
                <li><a href="{{ route('login') }}">ログイン</a></li>
                <li><a href="{{ route('register') }}">会員登録</a></li>
                @endif
            </ul>
        </nav>
    </header>
    <main class="container">

        @yield('content')

    </main>
    <footer>
        &copy; TanksG
    </footer>
</body>
</html>
