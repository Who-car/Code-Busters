<template>
    <div class="preloader">
        <div class="preloader__row">
            <div class="preloader__item"></div>
            <div class="preloader__item"></div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.preloader {
    position: fixed;
    left: 0;
    top: 0;
    right: 0;
    bottom: 0;
    z-index: 1000;
}

.preloader__row {
    position: relative;
    top: 50%;
    left: 50%;
    width: 70px;
    height: 70px;
    margin-top: -35px;
    margin-left: -35px;
    text-align: center;
    animation: preloader-rotate 2s infinite linear;
}

.preloader__item {
    border: 2px solid #000;
    position: absolute;
    display: inline-block;
    top: 0;
    background-color: $base-orange;
    width: 17px;
    height: 17px;

    &:last-child {
        top: auto;
        bottom: 0;
        animation-delay: -1s;
    }
}

@keyframes preloader-rotate {
    100% {
        transform: rotate(360deg);
    }
}
</style>
