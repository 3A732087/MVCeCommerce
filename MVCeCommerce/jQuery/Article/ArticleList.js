//畫面載入後事件
$(function () {
    //綁定新增文章按鈕點擊事件
    //因新增文章按鈕是後來用ajax取回產生，故採用$(document).delegate

    $(document).delegate('#CreateArticleModal #createBtn', 'click', function () {
        $('#CreateArticleModal form').submit();
    });
});