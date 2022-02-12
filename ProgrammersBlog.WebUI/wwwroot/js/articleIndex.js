

$(document).ready(function () {
    const dataTable = $('#articlesTable').DataTable({
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
                    let url = window.location.href;
                    url = url.replace('/Index', '');
                    window.open(`${url}/Add`);
                }
            },
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Article/GetAllArticles',
                        contentType: 'application/json',
                        beforeSend: function () {
                            $('#articlesTable').hide();
                            $('.spinner-border').show()
                        },
                        success: function (data) {

                            const articleListDto = jQuery.parseJSON(data)
                            console.log(articleListDto);
                            //dataTable.clear();

                            if (articleListDto.ResultStatus === 0) {
                                $.each(articleListDto.Articles.$values,
                                    function (index, article) {
                                        console.log(article.ArticleId);
                                    //    const newTableRow = dataTable.row.add([
                                    //        user.Id,
                                    //        user.UserName,
                                    //        user.Email,
                                    //        user.PhoneNumber,
                                    //        `<img src="/img/${user.Picture}" alt="${user.UserName}" class="my-image-table" />`,
                                    //        ` 
                                    //            <button class="btn btn-primary btn-sm btn-update" data-id="${user.Id}">
                                    //                <i class="fas fa-edit"></i>
                                    //            </button>
                                    //            <button class="btn btn-danger btn-sm btn-delete" data-id="${user.Id}">
                                    //                <i class="fas fa-minus-circle"></i>
                                    //            </button>
                                    //        `

                                    //    ]).node();
                                    //    const jqueryTableRow = $(newTableRow);
                                    //    jqueryTableRow.attr('name', user.Id);

                                    });

                                //dataTable.draw();
                                $('.spinner-border').hide();
                                $('#articlesTable').fadeIn(2500);
                            } else {

                                toastr.error(`${userListDto.Message}`, 'İşlem Başarısız!');
                            }

                        },
                        error: function (err) {
                            $('.spinner-border').hide();
                            $('#articlesTable').fadeIn(2500);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    })
                }
            }
        ],
        language: trlanguage
    });
    //DataTables ends here 

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

 

});
