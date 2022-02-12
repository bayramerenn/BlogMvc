$(document).ready(function () {
    const dataTable = $('#categoriesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        "order": [[6, "desc"]],
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
                        url: '/Admin/Category/GetAllCategories',
                        contentType: 'application/json',
                        beforeSend: function () {
                            $('#categoriesTable').hide();
                            $('.spinner-border').show()
                        },
                        success: function (data) {
                            const categoryListDto = jQuery.parseJSON(data)

                            dataTable.clear();

                            if (categoryListDto.ResultStatus === 0) {
                                console.log(categoryListDto);

                                $.each(categoryListDto.Categories.$values,
                                    function (index, category) {
                                        const newTableRow = dataTable.row.add([
                                            category.CategoryId,
                                            category.Name,
                                            category.Description,
                                            convertFirstLetterToUpperCase(category.IsActive.toString()),
                                            convertFirstLetterToUpperCase(category.IsDeleted.toString()),
                                            category.Note,
                                            convertToShortDate(category.CreatedDate),
                                            category.CreatedByName,
                                            convertToShortDate(category.ModifiedDate),
                                            category.ModifiedByName,
                                            ` <td>
                                                                <button class="btn btn-primary btn-sm btn-update" data-id="${category.CategoryId}">
                                                                     <i class="fas fa-edit"></i>
                                                                </button>
                                                                <button class="btn btn-danger btn-sm btn-delete" data-id="${category.CategoryId}">
                                                                    <i class="fas fa-minus-circle"></i>
                                                                </button>
                                                            </td>`
                                        ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', category.CategoryId)
                                    });
                                dataTable.draw();
                                $('.spinner-border').hide()
                                $('#categoriesTable').fadeIn(2500);
                            } else {
                                toastr.error(`${categoryListDto.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            $('.spinner-border').hide();
                            $('#categoriesTable').fadeIn(2500);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    })
                }
            }
        ],
        language: trlanguage
    });
    //DataTables ends here

    //          Category Added
    $(function () {
        const url = '/Admin/Category/Add/';
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
                const form = $('#form-category-add');
                const actionUrl = form.attr('action');
                const dataToSend = form.serialize();
                $.post(actionUrl, dataToSend).done(function (data) {
                    const categoryAddAjaxModel = jQuery.parseJSON(data)
                    console.log(categoryAddAjaxModel);
                    const newFormBody = $('.modal-body', categoryAddAjaxModel.CategoryAddPartial);

                    placeHolderDiv.find('.modal-body').replaceWith(newFormBody);

                    const isValid = newFormBody.find('[name="IsValid"').val() === 'True';
                    if (isValid) {
                        placeHolderDiv.find('.modal').modal('hide');
                        const newTableRow = dataTable.row.add([
                            categoryAddAjaxModel.CategoryDto.Category.CategoryId,
                            categoryAddAjaxModel.CategoryDto.Category.Name,
                            categoryAddAjaxModel.CategoryDto.Category.Description,
                            convertFirstLetterToUpperCase(categoryAddAjaxModel.CategoryDto.Category.IsActive.toString()),
                            convertFirstLetterToUpperCase(categoryAddAjaxModel.CategoryDto.Category.IsDeleted.toString()),
                            categoryAddAjaxModel.CategoryDto.Category.Note,
                            convertToShortDate(categoryAddAjaxModel.CategoryDto.Category.CreatedDate),
                            categoryAddAjaxModel.CategoryDto.Category.CreatedByName,
                            convertToShortDate(categoryAddAjaxModel.CategoryDto.Category.ModifiedDate),
                            categoryAddAjaxModel.CategoryDto.Category.ModifiedByName,
                            ` <td>
                                                                <button class="btn btn-primary btn-sm btn-update" data-id="${categoryAddAjaxModel.CategoryDto.Category.CategoryId}">
                                                                     <i class="fas fa-edit"></i>
                                                                </button>
                                                                <button class="btn btn-danger btn-sm btn-delete" data-id="${categoryAddAjaxModel.CategoryDto.Category.CategoryId}">
                                                                    <i class="fas fa-minus-circle"></i>
                                                                </button>
                                                            </td>`
                        ]).node();
                        const jqueryTableRow = $(newTableRow);
                        jqueryTableRow.attr('name', categoryAddAjaxModel.CategoryDto.Category.CategoryId)
                        dataTable.row(newTableRow).draw();

                        toastr.success(`${categoryAddAjaxModel.CategoryDto.Message}`, 'Başarılı İşlem!');
                    }
                    else {
                        let summaryText = "";
                        $('#validation-summary > ul > li').each(function () {
                            summaryText = `*${$(this).text()}\n`
                        });
                        toastr.warning(summaryText);
                    }
                });
            }
        )
    });

    /* Category deleted */
    $(document).on('click', '.btn-delete', function (event) {
        event.preventDefault();

        const id = $(this).attr('data-id');

        const tableRow = $(`[name="${id}"]`);
        const categoryName = tableRow.find('td:eq(1)').text();

        Swal.fire('Any fool can use a computer')
        Swal.fire({
            title: 'Silmek istediğinize emin misiniz?',
            text: `${categoryName} adlı kategori silinecektir!`,
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
                    data: { categoryId: id },
                    url: '/Admin/Category/Delete/',
                    success: function (data) {
                        const result = jQuery.parseJSON(data);

                        console.log(result);
                        if (result.ResultStatus === 0) {
                            Swal.fire(
                                'Silindi',
                                result.Message,
                                'success'
                            );

                            dataTable.row(tableRow).remove().draw()
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
    //Category Updated
    $(function () {
        const url = '/Admin/Category/Update/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $(document).on('click', '.btn-update',
            function (event) {
                event.preventDefault();
                const id = $(this).attr('data-id');

                $.get(url, { categoryId: id }).done(function (data) {
                    console.log(data);
                    placeHolderDiv.html(data);
                    placeHolderDiv.find(".modal").modal('show');
                }).fail(function (err) {
                    toastr.error('Hata')
                });
            });

        placeHolderDiv.on('click', '#btnUpdate',
            function (event) {
                event.preventDefault();

                const form = $('#form-category-update');
                const actionUrl = form.attr('action');
                const dataToSend = form.serialize();
                $.post(actionUrl, dataToSend).done(function (data) {
                    const categoryUpdateAjaxModel = jQuery.parseJSON(data);
                    
                    const newFormBody = $('.modal-body', categoryUpdateAjaxModel.CategoryUpdatePartial);
                    placeHolderDiv.find('.modal-body').replaceWith(newFormBody);
                    let tableRow;

                    if (categoryUpdateAjaxModel.CategoryDto !== null) {
                        const id = categoryUpdateAjaxModel.CategoryDto.Category.CategoryId;
                        tableRow = $(`[name="${id}"]`);
                    }

                    const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';

                    if (isValid) {
                        placeHolderDiv.find('.modal').modal('hide');


                        dataTable.row(tableRow).data([
                            categoryUpdateAjaxModel.CategoryDto.Category.CategoryId,
                            categoryUpdateAjaxModel.CategoryDto.Category.Name,
                            categoryUpdateAjaxModel.CategoryDto.Category.Description,
                            convertFirstLetterToUpperCase(categoryUpdateAjaxModel.CategoryDto.Category.IsActive.toString()),
                            convertFirstLetterToUpperCase(categoryUpdateAjaxModel.CategoryDto.Category.IsDeleted.toString()),
                            categoryUpdateAjaxModel.CategoryDto.Category.Note,
                            convertToShortDate(categoryUpdateAjaxModel.CategoryDto.Category.CreatedDate),
                            categoryUpdateAjaxModel.CategoryDto.Category.CreatedByName,
                            convertToShortDate(categoryUpdateAjaxModel.CategoryDto.Category.ModifiedDate),
                            categoryUpdateAjaxModel.CategoryDto.Category.ModifiedByName,
                            ` <td>
                                                                <button class="btn btn-primary btn-sm btn-update" data-id="${categoryUpdateAjaxModel.CategoryDto.Category.CategoryId}">
                                                                     <i class="fas fa-edit"></i>
                                                                </button>
                                                                <button class="btn btn-danger btn-sm btn-delete" data-id="${categoryUpdateAjaxModel.CategoryDto.Category.CategoryId}">
                                                                    <i class="fas fa-minus-circle"></i>
                                                                </button>
                                                            </td>`
                        ]);
                     
                        tableRow.attr('name', categoryUpdateAjaxModel.CategoryDto.Category.CategoryId)
                        dataTable.row(newTableRow).invalidate();
                       

                        toastr.success(categoryUpdateAjaxModel.CategoryDto.Message, 'Başarılı İşlem!');
                    } else {
                        let summaryText = '';
                        $('#validatin-summary > ul > li').each(function () {
                            let text = $(this).text();
                            summaryText = `${text}\n`;
                        })
                        toastr.warning(summaryText);
                    }
                }).fail(function (err) {
                    toastr.error(err)
                })
            })
    });
})