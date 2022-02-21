

$(document).ready(function () {
    const dataTable = $('#rolesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        buttons: [

            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Role/GetAllRoles',
                        contentType: 'application/json',
                        beforeSend: function () {
                            $('#rolesTable').hide();
                            $('.spinner-border').show()
                        },
                        success: function (data) {

                            const roleListDto = jQuery.parseJSON(data)
                            console.log(roleListDto);
                            dataTable.clear();


                            $.each(roleListDto.Roles.$values,
                                function (index, role) {
                                    const newTableRow = dataTable.row.add([
                                        role.Id,
                                        role.Name,
                                        

                                    ]).node();
                                    const jqueryTableRow = $(newTableRow);
                                    jqueryTableRow.attr('name', role.Id);

                                });

                            dataTable.draw();
                            $('.spinner-border').hide();
                            $('#rolesTable').fadeIn(2500);


                        },
                        error: function (err) {
                            $('.spinner-border').hide();
                            $('#rolesTable').fadeIn(2500);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    })
                }
            }
        ],
        language: trlanguage
    });
    //DataTables ends here 





});
