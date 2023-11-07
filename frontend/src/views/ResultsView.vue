<script setup lang="ts">
const myArray = [1, 2, 3, 4, 5];
</script>

<template>
    <div class="result">
        <div class="result__wrapper">
            <h1 class="result__title">
                <span>quiz:</span>
                <span>JavaScript</span>
            </h1>
            <div class="result__info">
                <div class="result__delimiter delimiter">
                    <div class="delimiter__line" />
                    <div class="delimiter__text">Quiz Information</div>
                    <div class="delimiter__line" />
                </div>
                <div class="result__content">
                    <div class="result__statistics">
                        <div><span>Score:</span><span>40%</span></div>
                        <div><span>Total:</span><span>10</span></div>
                        <div><span>Correct:</span><span>8</span></div>
                    </div>
                    <div class="result__progress">
                        <div class="result__progress-inner">10%</div>
                    </div>
                </div>
            </div>
            <div class="result__answers">
                <div class="result__delimiter delimiter">
                    <div class="delimiter__line" />
                    <div class="delimiter__text">Answers</div>
                    <div class="delimiter__line" />
                </div>
                <div v-for="answer in myArray" class="result__answer-container">
                    <h2 class="result__question">
                        <span>Question:</span>
                        <span
                            >Inside which HTML element do we put the
                            JavaScript?</span
                        >
                    </h2>
                    <div class="result__answer-container">
                        <div class="result__answer">
                            <div class="result__answer-check">given</div>
                            <span>script</span>
                        </div>
                        <div class="result__answer">
                            <div class="result__answer-check">correct</div>
                            <span>script</span>
                        </div>
                    </div>
                    <div class="result__answer-delimiter"></div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.result {
    .result__wrapper {
        .result__title {
            @include stroke(1px, #000);

            text-align: center;
            margin: 50px auto 50px auto;
            font-size: 36px;
            font-weight: 700;

            span:first-child {
                color: $base-orange;
            }
            span:last-child {
                color: $base-yellow;
            }
        }
        .result__info {
            .result__content {
                margin: 15px auto 30px auto;
                display: grid;
                grid-template-columns: 1fr 1fr;
                justify-items: center;
                align-items: center;

                .result__statistics {
                    div {
                        @include stroke(1px, #000);
                        font-size: 30px;
                        font-weight: 700;

                        span:first-child {
                            color: $base-orange;
                        }

                        span:last-child {
                            color: $base-yellow;
                        }
                    }
                }

                .result__progress {
                    border: 2px solid #000;
                    position: relative;
                    width: 100px;
                    height: 100px;
                    margin: 0.5rem;
                    overflow: hidden;
                    color: $base-orange;

                    background-image: conic-gradient(
                        $base-orange 10%,
                        $base-yellow 0
                    );

                    .result__progress-inner {
                        border: 2px solid #000;
                        font-weight: bold;

                        letter-spacing: 0px;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        position: absolute;
                        top: 50%;
                        left: 50%;
                        transform: translate(-50%, -50%);
                        width: 65px;
                        height: 65px;
                        background: $base-gray;
                        font-size: 25px;
                        font-weight: 300;
                        color: rgba(255, 255, 255, 0.75);
                    }
                }
            }
        }
        .result__answers {
            .result__answer-container {
                margin-bottom: 20px;
                .result__question {
                    @include stroke(1px, #000);

                    margin: 40px auto 30px auto;
                    font-size: 20px;
                    font-weight: bold;
                    text-align: center;

                    span:first-child {
                        color: $base-orange;
                    }
                    span:last-child {
                        color: $base-yellow;
                    }
                }

                .result__answer-container {
                    display: grid;
                    grid-template-columns: 1fr 1fr;
                    gap: 20px;

                    .result__answer {
                        position: relative;
                        width: 100%;
                        height: 132px;
                        background: $base-gray;
                        margin-bottom: 20px;

                        display: flex;
                        justify-content: center;
                        align-items: center;

                        font-size: 20px;
                        font-weight: bold;

                        &:nth-of-type(1) {
                            border: 3px solid #c24646;
                            color: #c24646;
                        }
                        &:nth-of-type(2) {
                            border: 3px solid $base-orange;
                            color: $base-orange;
                        }

                        span {
                            @include stroke(1px, #000);
                            padding: 10px 70px;
                        }
                        .result__answer-check {
                            position: absolute;
                            width: 150px;
                            text-align: center;
                            left: -50px;
                            transform: rotate(-90deg);
                        }
                    }
                }

                .result__answer-delimiter {
                    height: 5px;
                    width: 100%;
                    background: $base-yellow;
                }
            }
        }
    }
    .delimiter {
        display: flex;
        .delimiter__line {
            width: 100%;
            height: 5px;
            background: $base-yellow;
        }
        .delimiter__text {
            position: relative;
            bottom: 6px;
            margin: 0px 12px 0px 12px;
            color: $base-yellow;
            font-family: $base-text-font;
            font-size: 15px;
            font-style: normal;
            font-weight: 700;
            line-height: normal;
            text-wrap: nowrap;
        }
    }
}
</style>
