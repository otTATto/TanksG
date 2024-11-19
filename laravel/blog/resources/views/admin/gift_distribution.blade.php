@extends('layouts.app')

@section('content')
<div class="container">
    <h1>プレゼント配布</h1>

    @if(session('success'))
        <div class="alert alert-success">
            {{ session('success') }}
        </div>
    @endif

    <form action="{{ route('admin.gift_distribution.store') }}" method="POST">
        @csrf
        
        <!-- アイテム選択 -->
        <div class="form-group">
            <label for="item_id">アイテム</label>
            <select name="item_id" id="item_id" class="form-control" required>
                <option value="">アイテムを選択</option>
                @foreach($items as $item)
                    <option value="{{ $item->id }}" {{ old('item_id') == $item->id ? 'selected' : '' }}>
                        {{ $item->name }}
                    </option>
                @endforeach
            </select>
        </div>

        <!-- 配布数 -->
        <div class="form-group">
            <label for="quantity">配布数</label>
            <input type="number" name="quantity" id="quantity" class="form-control" required min="1" value="{{ old('quantity') }}">
        </div>

        <!-- 全体配布チェック -->
        <div class="form-group">
            <label for="is_global">
                <input type="checkbox" name="is_global" id="is_global" {{ old('is_global') ? 'checked' : '' }}> ユーザー全体に配布
            </label>
        </div>

        <!-- ユーザー選択（全体配布がチェックされていない場合） -->
        <div class="form-group" id="user_id_group">
            <label for="user_id">ユーザー</label>
            <select name="user_id" id="user_id" class="form-control">
                <option value="">ユーザーを選択</option>
                @foreach($users as $user)
                    <option value="{{ $user->id }}" {{ old('user_id') == $user->id ? 'selected' : '' }}>
                        {{ $user->name }}
                    </option>
                @endforeach
            </select>
        </div>

        <button type="submit" class="btn btn-primary">配布</button>
    </form>
</div>

<script>
    // 全体配布チェックの状態に応じて、ユーザー選択の表示を切り替え
    function toggleUserSelection() {
        var isGlobalChecked = document.getElementById('is_global').checked;
        var userIdGroup = document.getElementById('user_id_group');
        userIdGroup.style.display = isGlobalChecked ? 'none' : 'block';
    }

    // イベントリスナー設定
    document.getElementById('is_global').addEventListener('change', toggleUserSelection);

    // ページロード時の初期化
    window.onload = function () {
        toggleUserSelection(); // 初期状態をチェックして反映
    };
</script>

@endsection
