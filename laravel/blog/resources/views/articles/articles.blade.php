@foreach ($articles as $article)
<article class="article-item">
    <div class="article-title"><a href="{{ route('articles.show', $article) }}">{{ $article->title }}</a></div>
    <div class="article-info">
        {{ $article->created_at }}｜{{ $article->user->name }}
    </div>
</article>
@endforeach