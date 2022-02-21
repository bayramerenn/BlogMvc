

$(document).ready(function () {
    //datatable
    const dataTable = $('#articlesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
       
        "order": [[4, "desc"]],
       
        language: trlanguage
        
    });
    //datatable

    //chart.js

    $.get('/Admin/Article/GetAllByViewCount?isAscending=false&takeSize=10', function (data) {
        const articleResult = jsonRecursive.parse(data);
       
        console.log(articleResult);
        let viewCountContext = $('#viewCountChart');

        let viewCountChart = new Chart(viewCountContext,
            {
                type: 'bar',
                data: {
                    labels: articleResult.map(article => article.Title),
                    datasets: [
                        {
                            label: "Okunma Sayısı",
                            data: articleResult.map(article => article.ViewCount),
                            backgroundColor: ['#393E46', '#00ADB5'],
                            hoverBorderWidth: 4,
                            hoverBorderColor: 'black'
                        },
                        {
                            label: "Yorum Sayısı",
                            data: articleResult.map(article => article.CommentCount),
                            backgroundColor: ['#00ADB5', '#393E46'],
                            hoverBorderWidth: 4,
                            hoverBorderColor: 'black'
                        }
                    ]
                },
                options: {
                    plugins: {
                        legend: {
                            labels: {
                                font: {
                                    size: 20
                                }
                            }
                        }
                    }
                }
            });
    })
   

    const ctx = document.getElementById('test');
    const myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
            datasets: [{
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
});
