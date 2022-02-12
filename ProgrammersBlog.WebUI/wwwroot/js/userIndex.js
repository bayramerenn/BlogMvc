

$(document).ready(function () {
    const dataTable = $('#usersTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        buttons: [
            {
                text: 'Ekle',
                attr: {
                    id: "btnAdd"
                },
                className: 'btn btn-success',
                action: function (e, dt, node, config) {

                }
            },
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/User/GetAllUsers',
                        contentType: 'application/json',
                        beforeSend: function () {
                            $('#usersTable').hide();
                            $('.spinner-border').show()
                        },
                        success: function (data) {

                            const userListDto = jQuery.parseJSON(data)
                            console.log(userListDto);
                            dataTable.clear();

                            if (userListDto.ResultStatus === 0) {
                                $.each(userListDto.Users.$values,
                                    function (index, user) {
                                     const newTableRow =   dataTable.row.add([
                                            user.Id,
                                            user.UserName,
                                            user.Email,
                                            user.PhoneNumber,
                                            `<img src="/img/${user.Picture}" alt="${user.UserName}" class="my-image-table" />`,
                                            ` 
                                                <button class="btn btn-primary btn-sm btn-update" data-id="${user.Id}">
                                                    <i class="fas fa-edit"></i>
                                                </button>
                                                <button class="btn btn-danger btn-sm btn-delete" data-id="${user.Id}">
                                                    <i class="fas fa-minus-circle"></i>
                                                </button>
                                            `

                                     ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', user.Id);

                                    });
                                
                                dataTable.draw();
                                $('.spinner-border').hide();
                                $('#usersTable').fadeIn(2500);
                            } else {

                                toastr.error(`${userListDto.Message}`, 'İşlem Başarısız!');
                            }

                        },
                        error: function (err) {
                            $('.spinner-border').hide();
                            $('#usersTable').fadeIn(2500);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    })
                }
            }
        ],
        language: trlanguage
    });
    //DataTables ends here 

    // Category Added 
    $(function () {
        const url = '/Admin/User/Add/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $('#btnAdd').click(function () {
            $.get(url).done(function (data) {
                placeHolderDiv.html(data);
                placeHolderDiv.find(".modal").modal('show');
            })
        });

        placeHolderDiv.on('click',
            '#btnSave',
            function (event) {
                event.preventDefault();
                const form = $('#form-user-add');
                const actionUrl = form.attr('action');
                const dataToSend = new FormData(form.get(0));
                console.log(dataToSend);

                $.ajax({
                    url: actionUrl,
                    type: "POST",
                    data: dataToSend,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        const userAddAjaxModel = jQuery.parseJSON(data)
                        console.log(userAddAjaxModel);
                        const newFormBody = $('.modal-body', userAddAjaxModel.UserAddPartial);

                        placeHolderDiv.find('.modal-body').replaceWith(newFormBody);

                        const isValid = newFormBody.find('[name="IsValid"').val() === 'True';
                        if (isValid) {
                            placeHolderDiv.find('.modal').modal('hide');
                            const newTableRow =   dataTable.row.add([
                                userAddAjaxModel.UserDto.User.Id,
                                userAddAjaxModel.UserDto.User.UserName,
                                userAddAjaxModel.UserDto.User.Email,
                                userAddAjaxModel.UserDto.User.PhoneNumber,
                                `<img src="/img/${userAddAjaxModel.UserDto.User.Picture}" alt="${userAddAjaxModel.UserDto.User.UserName}" class="my-image-table" />`,
                                `
                                    <button class="btn btn-primary btn-sm btn-update" data-id="${userAddAjaxModel.UserDto.User.Id}">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                    <button class="btn btn-danger btn-sm btn-delete" data-id="${userAddAjaxModel.UserDto.User.Id}">
                                        <i class="fas fa-minus-circle"></i>
                                    </button>
                               `

                            ]).node();
                            const jqueryTableRow = $(newTableRow);
                            jqueryTableRow.attr('name', userAddAjaxModel.UserDto.User.Id);
                            dataTable.row(newTableRow).draw();

                            toastr.success(`${userAddAjaxModel.UserDto.Message}`, 'Başarılı İşlem!');
                        }
                        else {
                            let summaryText = "";
                            $('#validation-summary > ul > li').each(function () {
                                summaryText = `*${$(this).text()}\n`
                            });
                            toastr.warning(summaryText);
                        }
                    },
                    error: function (err) {
                        toastr.error(`${err.responseText}`, 'Hata!');
                    }
                });
            })

    }
    )

    /* Category deleted */
    $(document).on('click', '.btn-delete', function (event) {
        event.preventDefault();

        const id = $(this).attr('data-id');

        const tableRow = $(`[name="${id}"]`);
        const userName = tableRow.find('td:eq(1)').text();

        Swal.fire('Any fool can use a computer')
        Swal.fire({
            title: 'Silmek istediğinize emin misiniz?',
            text: `${userName} adlı kullanıcı silinecektir!`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Evet, silmek istiyorum.',
            calcelButtonText: 'Hayır, silmek istemiyorum.'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: { userId: id },
                    url: '/Admin/User/Delete/',
                    success: function (data) {
                        const result = jQuery.parseJSON(data);

                        console.log(result);
                        if (result.ResultStatus === 0) {
                            Swal.fire(
                                'Silindi',
                                result.Message,
                                'success'
                            );

                            dataTable.row(tableRow).remove().draw();
                        } else {
                            Swal.fire(
                                'Başarısız İşlem!',
                                result.Message,
                                'error'
                            );
                        }
                    },
                    error: function (err) {
                        toastr.error(`${err.responseText}`, 'Hata!')
                    }
                });
            }
        });
    });

    //user Updated
    $(function () {
        const url = '/Admin/User/Update/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $(document).on('click', '.btn-update',
            function (event) {

                event.preventDefault();
                const id = $(this).attr('data-id');

                $.get(url, { userId: id }).done(function (data) {

                    placeHolderDiv.html(data);
                    placeHolderDiv.find(".modal").modal('show');
                }).fail(function (err) {
                    toastr.error('Hata')
                });
            });

        placeHolderDiv.on('click', '#btnUpdate',
            function (event) {
                event.preventDefault();

                const form = $('#form-user-update');
                const actionUrl = form.attr('action');
                const dataToSend = new FormData(form.get(0));
                console.log(dataToSend);

                $.ajax({
                    url: actionUrl,
                    type: "POST",
                    data: dataToSend,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        const userUpdateAjaxModel = jQuery.parseJSON(data);
                        let tableRow;
                        if (userUpdateAjaxModel.UserDto !== null) {
                            const id = userUpdateAjaxModel.UserDto.User.Id;
                             tableRow = $(`[name="${id}"]`);
                        }
                       
                        const newFormBody = $('.modal-body', userUpdateAjaxModel.UserUpdatePartial);
                        placeHolderDiv.find('.modal-body').replaceWith(newFormBody);

                        const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                        if (isValid) {
                            placeHolderDiv.find('.modal').modal('hide');
                            dataTable.row(tableRow).data([
                                userUpdateAjaxModel.UserDto.User.Id,
                                userUpdateAjaxModel.UserDto.User.UserName,
                                userUpdateAjaxModel.UserDto.User.Email,
                                userUpdateAjaxModel.UserDto.User.PhoneNumber,
                                `<img src="/img/${userUpdateAjaxModel.UserDto.User.Picture}" alt="${userUpdateAjaxModel.UserDto.User.UserName}" class="my-image-table" />`,
                                `
                                    <button class="btn btn-primary btn-sm btn-update" data-id="${userUpdateAjaxModel.UserDto.User.Id}">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                    <button class="btn btn-danger btn-sm btn-delete" data-id="${userUpdateAjaxModel.UserDto.User.Id}">
                                        <i class="fas fa-minus-circle"></i>
                                    </button>
                               `

                            ]);
                            tableRow.attr('name', `${id}`);
                            dataTable.row(tableRow).invalidate();
                            toastr.success(userUpdateAjaxModel.UserDto.Message, 'Başarılı İşlem!');

                        } else {
                            let summaryText = '';
                            $('#validatin-summary > ul > li').each(function () {
                                let text = $(this).text();
                                summaryText = `${text}\n`;

                            })
                            toastr.warning(summaryText);
                        }
                    },
                    error: function (err) {
                        toastr.error(`${err.responseText}`,'Hata!');
                    }
                })
            });

    });

});
