function convertFirstLetterToUpperCase(text) {
    console.log(text);
    if (text === 'true') {
        console.log('deneme');
        return 'Evet'
    }
    return 'Hayır'
    //return text.charAt(0).toUpperCase() + text.slice(1);

}
function convertToShortDate(dateString) {
    const shortDate = new Date(dateString).toLocaleDateString('en-US');
    return shortDate;
}