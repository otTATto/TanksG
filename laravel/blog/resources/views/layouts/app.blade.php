<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>Document</title>
    <link rel="stylesheet" href="/main.css">
</head>
<body>
    <header>
        <a href="/" class="site-title">Laravel</a>
        <nav class="tab">
            <ul>
                @if (Auth::check())
                <li><a class="tab-item{{ Request::is('home') ? ' active' : ''}}" href="{{ route('home') }}">マイページ</a></li>
                <li><a class="tab-item{{ Request::is('articles') ? ' active' : ''}}" href="{{ route('articles.index') }}">お知らせ</a></li>
                <li><a class="tab-item{{ Request::is('contact') ? ' active' : ''}}" href="{{ route('contact.index') }}">お問い合わせ</a></li>
                
                @if (Auth::user()->is_admin) <!-- 管理者のみ表示 -->
                <li><a class="tab-item{{ Request::is('admin/game-users') ? ' active' : ''}}" href="{{ route('admin.game_users.index') }}">ゲームユーザー管理</a></li>
                <li><a class="tab-item{{ Request::is('admin/gift-logs') ? ' active' : ''}}" href="{{ route('admin.gift_logs.index') }}">プレゼント配布履歴</a></li>
                <li><a class="tab-item{{ Request::is('admin/gift-distribution') ? ' active' : ''}}" href="{{ route('admin.gift_distribution.create') }}">プレゼント配布</a></li>
                @endif
                
                <li>
                    <form onsubmit="return confirm('ログアウトしますか？')" action="{{ route('logout') }}" method="post">
                        @csrf
                        <button type="submit">ログアウト</button>
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
