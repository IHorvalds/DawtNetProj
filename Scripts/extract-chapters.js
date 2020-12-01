if ($(".chapter").length > 0) {
    $(".chapter").each(function (_, element) {
        console.log(element);
        var chapter = `<li><a href=#${element.getAttribute("id")}>${element.textContent}</a></li>`;
        $("#article_chapters").append(chapter);
    });
} else {
    $("#article_chapters").text("No chapters");
}
