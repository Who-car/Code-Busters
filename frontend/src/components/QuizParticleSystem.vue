<template>
    <div>
        <div class="animation-wrapper">
            <div class="particle particle-1"></div>
            <div class="particle particle-2"></div>
            <div class="particle particle-3"></div>
            <div class="particle particle-4"></div>
        </div>
    </div>
</template>

<style lang="scss" scoped>
/* Config */
$color-particle: #006871;
$spacing: 2560px;
$time-1: 120s;
$time-2: 240s;
$time-3: 360s;
$time-4: 600s;

/* Функция для генерации случайных чисел в заданном диапазоне */
@function random-range($min, $max) {
  $range: $max - $min;
  @return random() * $range + $min;
}

/* awesome mixin */
@function particles($max) {
  $val: 0px 0px $color-particle;
  @for $i from 1 through $max {
    $x: random-range(0px, $spacing);
    $y: random-range(0px, $spacing);
    $val: #{$val}, #{$x} #{$y} $color-particle;
  }
  @return $val;
}

@mixin particles($max) {
  box-shadow: particles($max);
}

.page-bg,
.animation-wrapper {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.particle,
.particle:after {
  background: transparent;
}

.particle:after {
  position: absolute;
  content: "";
  top: $spacing;
}

.particle-1 {
  animation: animParticle $time-1 linear infinite;
  @include particles(600);
  height: 2px;
  width: 2px;
}

.particle-1:after {
  @include particles(600);
  height: 2px;
  width: 2px;
}

.particle-2 {
  animation: animParticle $time-2 linear infinite;
  @include particles(200);
  height: 4px;
  width: 4px;
}

.particle-2:after {
  @include particles(200);
  height: 4px;
  width: 4px;
}

.particle-3 {
  animation: animParticle $time-3 linear infinite;
  @include particles(100);
  height: 5px;
  width: 5px;
}

.particle-3:after {
  @include particles(100);
  height: 5px;
  width: 5px;
}

.particle-4 {
  animation: animParticle $time-4 linear infinite;
  @include particles(400);
  height: 6px;
  width: 6px;
}

.particle-4:after {
  @include particles(400);
  height: 6px;
  width: 6px;
}

@keyframes animParticle {
  from { transform: translateY(0px); }
  to   { transform: translateY($spacing * -1); }
}
</style>
