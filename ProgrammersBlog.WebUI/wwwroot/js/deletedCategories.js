$(document).ready(function () {
    const dataTable = $('#deletedCategoriesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        "order": [[6, "desc"]],
        buttons: [
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Category/GetAllDeletedCategories/',
                        contentType: 'application/json',
                        beforeSend: function () {
                            $('#deletedCategoriesTable').hide();
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
                                            category.IsActive ? "Evet":"Hayır",
                                            category.IsDeleted ? "Evet":"Hayır",
                                            category.Note,
                                            convertToShortDate(category.CreatedDate),
                                            category.CreatedByName,
                                            convertToShortDate(category.ModifiedDate),
                                            category.ModifiedByName,
                                                        ` <td>
                                                                <button class="btn btn-warning btn-sm btn-undo" data-id="${category.CategoryId}">
                                                                     <i class="fas fa-undo"></i>
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
                                $('#deletedCategoriesTable').fadeIn(2500);
                            } else {
                                toastr.error(`${categoryListDto.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            $('.spinner-border').hide();
                            $('#deletedCategoriesTable').fadeIn(2500);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    })
                }
            }
        ],
        language: trlanguage
    });
    //DataTables ends here

    /* UndoDelete */

    $(document).on('click',
        '.btn-undo',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            let categoryName = tableRow.find('td:eq(1)').text();
           
            Swal.fire({
                title: 'Arşivden geri getirmek istediğinize emin misiniz?',
                text: `${categoryName} adlı kategori arşivden geri getirelecektir!`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, arşivden geri getirmek istiyorum.',
                cancelButtonText: 'Hayır, istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { categoryId: id },
                        url: '/Admin/Category/UndoDelete/',
                        success: function (data) {
                            const undoDeletedCategoryResult = jQuery.parseJSON(data);
                            
                            if (undoDeletedCategoryResult.ResultStatus === 0) {
                                Swal.fire(
                                    'Arşivden Geri Getirildi!',
                                    `${undoDeletedCategoryResult.Message}`,
                                    'success'
                                );

                                dataTable.row(tableRow).remove().draw();
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `${undoDeletedCategoryResult.Message}`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!");
                        }
                    });
                }
            });
        });
    /* UndoDelete */

    /* HardDelete */

    $(document).on('click',
        '.btn-delete',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            let categoryName = tableRow.find('td:eq(1)').text();

            Swal.fire({
                title: 'Kalıcı olarak silmek istediğinize emin misiniz?',
                text: `${categoryName} adlı kategori kalıcı olarak silinecektir!`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, kalıcı olarak silmek istiyorum.',
                cancelButtonText: 'Hayır, istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { categoryId: id },
                        url: '/Admin/Category/HardDelete/',
                        success: function (data) {
                            const hardDeleteResult = jQuery.parseJSON(data);

                            if (hardDeleteResult.ResultStatus === 0) {
                                Swal.fire(
                                    'Kalıcı olarak silindi!',
                                    `${hardDeleteResult.Message}`,
                                    'success'
                                );

                                dataTable.row(tableRow).remove().draw();
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `${hardDeleteResult.Message}`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!");
                        }
                    });
                }
            });
        });
    /* UndoDelete */
   
})