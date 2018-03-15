$(function () {

    var idAccumulator = 1;

    $("#addTaskHandler").click(() => {
        $("#TaskList").append(`<li><input class="form-control text-box single-line" id="CustomTaskItem_${idAccumulator}__ItemName" name="CustomTaskItem[${idAccumulator}].ItemName" type="text" value=""></li>`);
        idAccumulator++;
    });

});