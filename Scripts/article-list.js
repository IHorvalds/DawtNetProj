$(".table_button").on("click", function () {
    let newUrl = $(this).attr("data-url");
    if (typeof newUrl != undefined) {
        window.location = window.location.protocol + '//' + window.location.host + newUrl;
    }
});