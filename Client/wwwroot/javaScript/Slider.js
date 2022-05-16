const sliders = document.querySelector(".carouselBox")

var scrollPerClick;
var imagePadding = 20;

showMovieData()






async function showMovieData() {
    const api_key = "a5ab4805002668ee4999f8bac7a4691d";
    
    var result = await axios.get(
        "https://api.themoviedb.org/3/trending/movie/week?api_key=" + api_key
    )
    
    result = result.data.results;
    result.map(function (cur, index){
        sliders.insertAdjacentHTML(
            "beforeend",
            '<img=class="img-${index} slider-img" src="https://image.tmdb.org/t/p/w185/${cur.poster_path}" />'
        )
    })
    
    scrollPerClick = document.querySelector(".img-1").clientWidth + imagePadding;
}