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

                           
                            const articleListDto = jsonRecursive.parse(data);
                            
                           
                            //const articleListDto = jQuery.parseJSON(data)

                            console.log(articleListDto);
                            //dataTable.clear();

                            if (articleListDto.ResultStatus === 0) {
                                $.each(articleListDto.Articles,
                                    function (index, article) {
                                        console.log(article.Category.Name);
                                      
                                        const newTableRow = dataTable.row.add([
                                            article.ArticleId,
                                            article.Category.Name,
                                            article.Title,
                                            `<img src="/img/${article.Thumbnail}" alt="${article.Title}" class="my-image-table" />`,
                                            `${convertToShortDate(article.Date)}`,
                                            article.CommentCount,
                                            article.ViewCount,
                                            `${article.IsActive ? "Evet" : "Hayır"}`,
                                            `${article.IsDeleted ? "Evet" : "Hayır"}`,
                                            `${convertToShortDate(article.CreatedDate)}`,
                                            article.CreatedByName,
                                            `${convertToShortDate(article.ModifiedDate)}`,
                                            article.ModifiedByName,

                                            `
                                                <a class="btn btn-primary btn-sm btn-update" asp-area="Admin" asp-action="Update" asp-controller="Article" asp-route-articleId="${article.ArticleId}">
                                                    <i class="fas fa-edit"></i>
                                                </a>
                                                <button class="btn btn-danger btn-sm btn-delete" data-id="${article.ArticleId}">
                                                    <i class="fas fa-minus-circle"></i>
                                                </button>
                                            `

                                        ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', article.ArticleId);
                                    });

                                //dataTable.draw();
                                $('.spinner-border').hide();
                                $('#articlesTable').fadeIn(2500);
                            } else {
                                toastr.error(`${articleListDto.Message}`, 'İşlem Başarısız!');
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
        const articleTitle = tableRow.find('td:eq(2)').text();
        console.log(articleTitle);
        Swal.fire('Any fool can use a computer')
        Swal.fire({
            title: 'Silmek istediğinize emin misiniz?',
            text: `${articleTitle} başlıklı makale silinecektir!`,
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
                    data: { articleId: id },
                    url: '/Admin/Article/Delete/',
                    success: function (data) {
                        const articleResult = jQuery.parseJSON(data);

                        console.log(result);
                        if (articleResult.ResultStatus === 0) {
                            Swal.fire(
                                'Silindi',
                                articleResult.Message,
                                'success'
                            );

                            dataTable.row(tableRow).remove().draw();
                        } else {
                            Swal.fire(
                                'Başarısız İşlem!',
                                articleResult.Message,
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