$("#selected_version").on('change', function () {
    let selectedLink = $("#selected_version").val();
    if (typeof selectedLink != undefined) {
        window.location.pathname = selectedLink;
    }
});