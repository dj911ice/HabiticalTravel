$(function () {
    $('.my_modal').on('show.bs.modal', function (e) {
        var itemId = $(e.relatedTarget).data('item-id');
        $(e.currentTarget).find('a[name="taskId"]').attr("href", itemId);
    });
});